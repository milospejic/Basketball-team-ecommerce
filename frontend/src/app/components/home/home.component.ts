import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { Product } from '../../models/product';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  productList: Product[] = [];
  searchQuery: string = '';
  currentPage: number = 1;
  pageSize: number = 10;
  sortBy: string = 'price';
  sortOrder: string = 'asc';
  showSortingAndPagination: boolean = true;

  
  constructor(private productService: ProductService, private router: Router) {}

  ngOnInit(): void {
    this.loadAllProducts();
  }

  loadAllProducts(): void {
    this.showSortingAndPagination = true;

    this.productService.getAllProducts(this.currentPage, this.pageSize, this.sortBy, this.sortOrder).subscribe(
      (result: Product[]) => {
        this.productList = result;
        console.log(result);
        if(this.productList == null){
          this.currentPage--;
          this.loadAllProducts();
        }
      },
      (error) => {
        console.error('Error loading products:', error);
      }
    );
  }

  loadProductsByCategory(category: string): void {
    this.showSortingAndPagination = false;

    this.productService.getAllProductsFull().subscribe(
      (result: Product[]) => {
        this.productList = result.filter((product) => product.category === category);
      },
      (error) => {
        console.error('Error loading products:', error);
      }
    );
  }

  searchProducts(): void {
    if (this.searchQuery.trim() !== '') {
      this.productService.searchProducts(this.searchQuery).subscribe(
        (result: Product[]) => {
          this.productList = result;

        },
        (error) => {
          console.error('Error searching products:', error);
        }
      );
    } else {
      this.loadAllProducts();
    }
  }

  addProductToCart(id: string): void {
    this.productService.addToCart(id);
    console.log('Product added to cart:', id);
  }

  removeProductFromCart(id: string): void {
    this.productService.removeFromCart(id);
    console.log('Product removed from cart:', id);
  }

  viewProductDetails(id: string): void {
    this.router.navigate(['/product', id]);
  }

  userRole(): string | null {
    return localStorage.getItem('userRole');
  }

  onNextPage() {
    //if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.loadAllProducts();
    //}
  }

  onPreviousPage() {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadAllProducts();
    }
  }
}
