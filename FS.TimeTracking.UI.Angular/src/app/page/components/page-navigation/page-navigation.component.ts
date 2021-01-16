import {Component, OnInit} from '@angular/core';
import {environment} from "../../../../environments/environment";

@Component({
  selector: 'page-navigation',
  templateUrl: './page-navigation.component.html',
  styleUrls: ['./page-navigation.component.scss']
})
export class PageNavigationComponent implements OnInit {
  isDevelopment: boolean;

  constructor() {
    this.isDevelopment = !environment.production;
  }

  ngOnInit(): void {
  }

}
