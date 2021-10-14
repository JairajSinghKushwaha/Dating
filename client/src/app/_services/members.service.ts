import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root'
})

export class MembersService {
baseUrl = environment.baseUrl;
members: Member[] = [];

  constructor(private http: HttpClient) { }

  getMembers() {
    // 1. If members have the then return the member.
    if(this.members.length > 0) 
     return of(this.members);

     // 2. otherwise call the API to get the data 
     return this.http.get<Member[]>(this.baseUrl + 'users').pipe( map(members => { 
     this.members = members;
     return members;
   }));
  }

  getMember(userName: string): Observable<Member> {
    // // 1. check the member if not undefined then return the data
    const member = this.members.find(x=>x.userName === userName);
    if(member !== undefined) {
      console.log('storage data')
       return of(member);
    }
    // 2. otherwise call the API to get the data 
    console.log('API data')
    return this.http.get<Member>(this.baseUrl + 'users/'+ userName);
  }

  updateMember(member: Member) {
    // update the user
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(()=> {
        // update the member data after modify from the submited model.
        const index = this.members.indexOf(member); // get the index of old model index in the list.
        this.members[index] = member; // pass the latest data after geting the index. 
      })
    )
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId, {});
  }
}
