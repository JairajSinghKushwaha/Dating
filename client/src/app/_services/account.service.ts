import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import {map} from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

// decorator
@Injectable({
  providedIn: 'root'
})
export class AccountService {
baseUrl = environment.baseUrl;
private currentUserSource = new ReplaySubject<User|null>(1); // The size of the buffer is 1.
currentUser$ = this.currentUserSource.asObservable();
  constructor(private http: HttpClient) { }
  
  public login(model:any) {
    // The model contains userName and password.
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
          const user = response;
          if(user) {
            this.setCurrentUser(user);
          }
        })
    );
  }
  
  public register(model:any) {
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map((user: User) => {
        this.setCurrentUser(user);
      })   
    );
  }

  setCurrentUser(user: User) {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }

  public logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }
}
