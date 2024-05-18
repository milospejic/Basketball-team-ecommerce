import { NgModule} from '@angular/core';
import { BrowserModule, provideClientHydration } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './components/home/home.component';
import { CartComponent } from './components/cart/cart.component';
import { SaleComponent } from './components/sale/sale.component';


import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { ContactusComponent } from './components/contactus/contactus.component';
import { OrdersComponent } from './components/orders/orders.component';
import { MyProductsComponent } from './components/my-products/my-products.component';
import { AllOrdersComponent } from './components/all-orders/all-orders.component';
import { ProfileComponent } from './components/profile/profile.component';
import { AddressComponent } from './components/address/address.component';
@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    CartComponent,
    SaleComponent,
    LoginComponent,
    RegisterComponent,
    ContactusComponent,
    OrdersComponent,
    MyProductsComponent,
    AllOrdersComponent,
    ProfileComponent,
    AddressComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule
  ],
  providers: [
    provideClientHydration()
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
