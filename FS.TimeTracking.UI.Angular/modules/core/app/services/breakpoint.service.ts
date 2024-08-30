import {Injectable, OnDestroy} from '@angular/core';
import {BehaviorSubject, debounceTime, fromEvent, map, Observable, Subscription} from "rxjs";
import {startWith, tap} from "rxjs/operators";

export enum Breakpoint {
  xs = 0,
  sm = 576,
  md = 768,
  lg = 992,
  xl = 1200,
  xxl = 1400
}

@Injectable({
  providedIn: 'root'
})
export class BreakpointService {
  private breakpointValues: number[];

  public breakpoint$: Observable<Breakpoint>;

  constructor() {
    this.breakpointValues = this.getBreakpointValues();

    this.breakpoint$ = fromEvent(window, 'resize')
      .pipe(
        startWith(this.getBreakpoint()),
        debounceTime(10),
        map(_ => this.getBreakpoint())
      );
  }

  public getBreakpoint(): Breakpoint {
    return <Breakpoint>this.breakpointValues.find((value) => window.innerWidth >= value);
  }

  private getBreakpointValues(): number[] {
    return Object
      .values(Breakpoint)
      .filter(value => typeof value === "number")
      .map((value): number => <number>value)
      .sort((a, b) => b - a);
  }
}
