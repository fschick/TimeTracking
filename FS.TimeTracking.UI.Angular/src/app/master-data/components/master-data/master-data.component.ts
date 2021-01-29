import {AfterViewInit, Component, ElementRef, OnDestroy, ViewChild} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {Subscription} from 'rxjs';
import {Collapse} from 'bootstrap';

@Component({
  selector: 'ts-master-data',
  templateUrl: './master-data.component.html',
  styleUrls: ['./master-data.component.scss']
})
export class MasterDataComponent implements AfterViewInit, OnDestroy {
  @ViewChild('customers') private customers!: ElementRef;
  @ViewChild('projects') private projects!: ElementRef;
  @ViewChild('activities') private activities!: ElementRef;

  private subscriptions = new Subscription();

  constructor(public route: ActivatedRoute) {
  }

  ngAfterViewInit(): void {
    const accordionItems = [
      {entity: 'customers', collapse: new Collapse(this.customers.nativeElement, {toggle: false})},
      {entity: 'projects', collapse: new Collapse(this.projects.nativeElement, {toggle: false})},
      {entity: 'activities', collapse: new Collapse(this.activities.nativeElement, {toggle: false})}
    ];

    const routeParams = this.route.params.subscribe(param => {
      accordionItems.filter(item => item.entity !== param.entity).forEach(item => item.collapse.hide());
      accordionItems.find(item => item.entity === param.entity)?.collapse.show();
    });

    this.subscriptions.add(routeParams);
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }
}
