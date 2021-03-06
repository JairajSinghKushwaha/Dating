import { Component, OnInit } from '@angular/core';
import { User } from './_models/user';
import { AccountService } from './_services/account.service';

// Decorator
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent implements OnInit {
  title = 'Dating App!';
  users: any;
  apiData: any;
  constructor(private accountService: AccountService){
  }

  ngOnInit(): void {   
   this.setCurrentUser();
  }

  setCurrentUser() {
    const user: User = JSON.parse(localStorage.getItem('user')!);
    if(user)
    {
      this.accountService.setCurrentUser(user);
    }
  }
}
