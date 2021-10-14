import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { Photo } from 'src/app/_models/photo';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})

export class PhotoEditorComponent implements OnInit {
  @Input() member!: Member;
  uploader!: FileUploader;
  hasBaseDropZoneOver!: false;
  baseUrl = environment.baseUrl; 
  user!: User;

  constructor(private accountService: AccountService, private toastr: ToastrService, private memberService: MembersService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);
   }

  ngOnInit(): void {
    this.initializaUploader();
  }

  fileOverBase(e: any) {
    console.log('event: ' + e)
    this.hasBaseDropZoneOver = e;
  }
  
  setMainPhoto(photo: Photo) {
   this.memberService.setMainPhoto(photo.id).subscribe(() => {
    this.user.mainPhotoUrl = photo?.url;
    this.accountService.setCurrentUser(this.user!);
    this.member.photoUrl = photo.url;
    this.member.photos.forEach(p => {
        if(p.isMain) {
          console.log("set photo user: " + p.id)
          p.isMain = false;
        }
        if(p.id == photo.id) {
          console.log("user Id: " + p.id + ", photo id: " + photo.id)
          p.isMain = true;
        }
    })
   })
  }

  deletePhoto(photoId: number) {
    this.memberService.deletePhoto(photoId).subscribe(() => {
        this.member.photos = this.member.photos.filter(x => x.id !== photoId);
        this.toastr.success('Photo deleted successfully.');
      }
    );
  }

  initializaUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/add-photo',
      authToken: 'Bearer ' + this.user?.token,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });
    this.uploader.onAfterAddingFile = (file) => { file.withCredentials = false; };
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if(response) {
        const photo = JSON.parse(response);
        this.member.photos.push(photo);
        this.toastr.success('Photo uploaded successfully.');
      }
    }
  }
}
