import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { TableCellComponent } from './table-cell/table-cell.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {MatTableModule} from '@angular/material/table';
import { CacheComponent } from './cache/cache.component';
import { CpuComponent } from './cpu/cpu.component';
import { PrincipalBusComponent } from './principal-bus/principal-bus.component';
import { SecondaryBusComponent } from './secondary-bus/secondary-bus.component';

@NgModule({
  declarations: [
    AppComponent,
    TableCellComponent,
    CacheComponent,
    CpuComponent,
    PrincipalBusComponent,
    SecondaryBusComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule, 
    MatTableModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
