import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})

export class RegisterComponent implements OnInit {
model: any = {}
registerForm!: FormGroup;
@Output() cancleRegister = new EventEmitter();
  constructor(private accountService: AccountService, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.registerForm = new FormGroup({
      userName: new FormControl('aisha', Validators.required),
      password: new FormControl('', [Validators.required, Validators.minLength(6),Validators.maxLength(8)]),
      confirmPassword: new FormControl('', [Validators.required, this.matchValues('password')])
    });

    // Reset the confirmPassword when password changes.
    this.registerForm.controls.password.valueChanges.subscribe(() => { 
      this.registerForm.controls.confirmPassword.updateValueAndValidity()
    });
  }

   matchValues(matchTo: string) : ValidatorFn {
     return (control: AbstractControl) => {
       // if not match then return null otherwise return isMatching: true.
       // comparing confirmPassword === password
       return control?.value === (control?.parent?.controls as { [key: string]: AbstractControl })[matchTo]?.value ? null : { isMatching: true };
     }
   }

  register() {
    console.log(this.registerForm.value);
    // this.accountService.register(this.model).subscribe(response => {
    //   console.log(response);
    //   this.cancel();
    // }, error => {
    //   console.log(error);
    //   this.toastr.error(error.error)
    // });
  }

  cancel()
  {
    this.cancleRegister.emit(false);
  }
}
