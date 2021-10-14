import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, CanDeactivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<unknown> {
  canDeactivate(component: MemberEditComponent): boolean {
    if(component.editForm.dirty) {
       // If we click yes then it will return true after the the if block.
       // If select no then still stay on the form.
       return confirm('Are you sure you want to continue? Any unsaved changes will be lost.');
    }
    return true;
  }
  
}
