import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { CurrentUser } from '../models/currentUser';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private tokenKey = 'token';
  private loggedInKey = 'isLoggedIn';
  private roleKey = 'userRole';
  role!: string;

  constructor(private http: HttpClient) {}

  isLoggedIn(): boolean {
    return typeof localStorage !== 'undefined' && localStorage.getItem(this.loggedInKey) === 'true';
  }

  setLoggedIn(value: boolean): void {
    localStorage.setItem(this.loggedInKey, value.toString());
  }

  getUserRole(): string | null {
    return typeof localStorage !== 'undefined' ? localStorage.getItem(this.roleKey) : null;
  }

  setUserRole(role: string): void {
    localStorage.setItem(this.roleKey, role);
  }

  register(user: any): Observable<any> {
    return this.http.post<any>('http://localhost:5259/api/user', user);
  }

  login(email: string, password: string): Observable<boolean> {
    return this.http.post<any>('http://localhost:5259/api/Auth/authenticate', { email, password }).pipe(
      map(response => {
        if (response && response.token) {
          localStorage.setItem(this.tokenKey, response.token);
          this.setLoggedIn(true);
          this.setUserRole(response.role);  // Set the user role upon login
          return true;
        }
        return false;
      })
    );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.loggedInKey);
    localStorage.removeItem(this.roleKey);
  }
  

  getCurrentUser(): Observable<CurrentUser> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    });
    return this.http.get<CurrentUser>('http://localhost:5259/api/user/current-user',{headers}).pipe(
      map(response => {
        if (response && response.role) {
          this.setUserRole(response.role);
        }
        return response;
      })
    );
  }
}
