import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Review } from '../models/review';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ReviewService {

  constructor(private http: HttpClient) { }

  getAllReviews(): Observable<any>{
    return this.http.get<any>(`${environment.apiUrl}/api/review`);
  }

  getReviewsById(id: string): Observable<any>{
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    });
    return this.http.get<any>(`${environment.apiUrl}/api/review/${id}`, {headers});
  }

  addReview(review: Review): Observable<any> {
    console.log(localStorage.getItem('token'));
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    });
    return this.http.post<any>(`${environment.apiUrl}/api/review`, review, { headers });
  }
  updateReview(review: Review): Observable<any>{
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    });
    return this.http.put<any>(`${environment.apiUrl}/api/review/${review.reviewId}`, review, {headers});
  }

  deleteReview(id: string): Observable<any>{
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    });
    return this.http.delete<any>(`${environment.apiUrl}/api/review/${id}`, {headers});
  }

  getReviewsByProductId(id: string): Observable<Review[]> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    });
    return this.http.get<Review[]>(`${environment.apiUrl}/api/review/product/${id}`, {headers });
  }
}
