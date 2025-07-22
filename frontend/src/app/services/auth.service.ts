import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { CurrentUser } from '../models/currentUser';
import { isLocalStorageAvailable } from '../utils/localStorageUtil';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private tokenKey = 'token';
  private loggedInKey = 'isLoggedIn';
  private roleKey = 'userRole';
  private loggedIn = new BehaviorSubject<boolean>(false);

  role!: string;

  constructor(private http: HttpClient) {
    this.initializeLoginStatus();
  }

  get isLoggedIn() {
    return this.loggedIn.asObservable();
  }

  private initializeLoginStatus(): void {
    if (isLocalStorageAvailable()) {
      const loggedIn = localStorage.getItem(this.loggedInKey) === 'true';
      this.loggedIn.next(loggedIn);
    }
  }

  setLoggedIn(value: boolean): void {
    if (isLocalStorageAvailable()) {
      localStorage.setItem(this.loggedInKey, value.toString());
    }
  }

  getUserRole() {
    if (isLocalStorageAvailable()) {
      return localStorage.getItem(this.roleKey);
    }
    return null;
  }

  setUserRole(role: string): void {
    if (isLocalStorageAvailable()) {
      localStorage.setItem(this.roleKey, role);
    }
  }

  register(user: any): Observable<any> {
    return this.http.post<any>(`${environment.apiUrl}/api/user`, user);
  }

  login(email: string, password: string): Observable<boolean> {
    return this.http.post<any>(`${environment.apiUrl}/api/Auth/authenticate`, { email, password }).pipe(
      map(response => {
        if (response && response.token) {
          if (isLocalStorageAvailable()) {
            localStorage.setItem(this.tokenKey, response.token);
          }
          this.setLoggedIn(true);
          this.loggedIn.next(true);
          return true;
        }
        return false;
      })
    );
  }

  logout(): void {
    if (isLocalStorageAvailable()) {
      localStorage.removeItem(this.tokenKey);
      localStorage.removeItem(this.loggedInKey);
      localStorage.removeItem(this.roleKey);
    }
    this.loggedIn.next(false);
  }

  getCurrentUser(): Observable<CurrentUser> {
    let token = null;
    if (isLocalStorageAvailable()) {
      token = localStorage.getItem(this.tokenKey);
    }

    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    });

    return this.http.get<CurrentUser>(`${environment.apiUrl}/api/user/current-user`, { headers }).pipe(
      map(response => {
        if (response && response.role) {
          this.setUserRole(response.role);
        }
        return response;
      })
    );
  }
}
