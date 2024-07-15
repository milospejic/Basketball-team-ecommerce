import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { ReviewService } from '../../services/review.service';
import { Product } from '../../models/product';
import { Review } from '../../models/review';
import { AuthService } from '../../services/auth.service';
import { CurrentUser } from '../../models/currentUser';
import { MatSnackBar } from '@angular/material/snack-bar';


@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.css']
})
export class ProductDetailsComponent implements OnInit {
  product!: Product;
  reviews: Review[] = [];
  currentUser!: CurrentUser;
  amount!:number;


  constructor(
    private productService: ProductService,
    private reviewService: ReviewService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      const id = params['id'];
      this.loadProductDetails(id);
      this.loadReviewsByProductId(id);
      this.loadCurrentUser();
    });
  }

  loadProductDetails(id: string): void {
    this.productService.getProductById(id).subscribe(
      (product: Product) => {
        this.product = product;
      },
      (error) => {
        console.error('Error loading product details:', error);
      }
    );
  }

  loadReviewsByProductId(id: string): void {
    this.reviewService.getReviewsByProductId(id).subscribe(
      (reviews: Review[]) => {
        this.reviews = reviews;
      },
      (error) => {
        console.error('Error loading reviews:', error);
      }
    );
  }

  addProductToCart(id: string): void {
    this.amount = this.productService.getCartItemAmount(id);
    if (this.amount >= this.product.quantity) {
      this.snackBar.open('Amount exceeds product quantity', 'Close', {
        duration: 3000, // Duration in milliseconds
      });
    } else {
      this.productService.addToCart(id);
      console.log('Product added to cart:', id);
    }
  }

  removeProductFromCart(id: string): void {
    this.productService.removeFromCart(id);
    console.log('Product removed from cart:', id);
  }

  loadCurrentUser(): void {
    this.authService.getCurrentUser().subscribe(
      (user: CurrentUser) => {
        this.currentUser = user;
      },
      (error) => {
        console.error('Error loading current user:', error);
      }
    );
  }

  addReview(reviewText: string, ratingValue: any): void {
    if (!this.currentUser) {
      console.error('Current user not found.');
      return;
    }
    const rating = Number(ratingValue);



    const existingReview = this.reviews.find(review => review.userId === this.currentUser.id);
    if (existingReview) {
      existingReview.reviewText = reviewText;
      existingReview.rating = rating;
      this.reviewService.updateReview(existingReview).subscribe(
        () => {
          console.log('Review updated successfully.');
        },
        (error) => {
          console.error('Error updating review:', error);
        }
      );
    } else {
      const newReview: Review = {
        reviewId: '', 
        reviewText,
        rating,
        userId: this.currentUser.id,
        productId: this.product.productId 
      };
      this.reviewService.addReview(newReview).subscribe(
        () => {
          console.log('Review added successfully.');
          this.loadProductDetails(this.product.productId);
          this.loadReviewsByProductId(this.product.productId);

        },
        (error) => {
          console.error('Error adding review:', error);
        }
      );
    }
  }
  
  editReview(review: Review): void {
    const updatedReviewText = prompt('Edit your review:', review.reviewText);
    if (updatedReviewText !== null) {
      const updatedRating = Number(prompt('Update your rating (1-5):', String(review.rating)));
      if (!isNaN(updatedRating) && updatedRating >= 1 && updatedRating <= 5) {
        review.reviewText = updatedReviewText;
        review.rating = updatedRating;
        this.reviewService.updateReview(review).subscribe(
          () => {
            console.log('Review updated successfully.');
            this.loadProductDetails(this.product.productId);
            this.loadReviewsByProductId(this.product.productId);
          },
          (error) => {
            console.error('Error updating review:', error);
          }
        );
      } else {
        console.error('Invalid rating. Rating must be a number between 1 and 5.');
      }
    }
  }
  
  
  deleteReview(reviewId: string): void {
    if (confirm('Are you sure you want to delete this review?')) {
      this.reviewService.deleteReview(reviewId).subscribe(
        () => {
          console.log('Review deleted successfully.');
          this.loadProductDetails(this.product.productId);
          this.loadReviewsByProductId(this.product.productId);
        },
        (error) => {
          console.error('Error deleting review:', error);
        }
      );
    }
  }
  

  hasUserReviewed(): boolean {
    if (!this.currentUser) {
      return false;
    }
  
    return this.reviews.some(review => review.userId === this.currentUser.id);
  }
}
