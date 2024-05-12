import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, map, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private loggedInKey = 'isLoggedIn';
  constructor(private http: HttpClient) {}

  isLoggedIn(): Observable<boolean> {
    return of(localStorage.getItem(this.loggedInKey) === 'true');
  }

  setLoggedIn(value: boolean): void {
    localStorage.setItem(this.loggedInKey, value.toString());
  }

  register(user: any): Observable<any> {
    return this.http.post<any>('http://localhost:5259/api/user', user);
  }

  login(email: string, password: string): Observable<boolean> {
    return this.http.post<any>('http://localhost:5259/api/Auth/authenticate', { email, password }).pipe(
      map(response => {
        if (response && response.token) {
          localStorage.setItem('token', response.token);
          this.setLoggedIn(true); // Set logged in state to true
          return true;
        }
        return false;
      }),
      catchError(() => {
        this.setLoggedIn(false); // Set logged in state to false
        return of(false);
      })
    );
  }
}
