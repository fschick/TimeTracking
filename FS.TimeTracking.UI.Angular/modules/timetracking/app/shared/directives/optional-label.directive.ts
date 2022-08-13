import {Directive, ElementRef, Inject, Input, OnInit, Optional, Renderer2} from '@angular/core';
import {DOCUMENT} from '@angular/common';

export interface OptionalLabelOptions {
  short?: boolean;
}

@Directive({
  selector: '[tsOptionalLabel]'
})
export class OptionalLabelDirective implements OnInit {
  @Input() @Optional() tsOptionalLabel?: OptionalLabelOptions;

  constructor(
    private elementRef: ElementRef,
    private renderer: Renderer2,
    @Inject(DOCUMENT) private document: Document
  ) {
  }

  public ngOnInit(): void {
    const child = this.document.createElement('span');
    const optionalText =  $localize`:@@Common.Optional:[i18n] optional`;
    const optionalTextShort =  $localize`:@@Common.OptionalShort:[i18n] opt.`;
    child.innerText = this.tsOptionalLabel?.short ? ` (${optionalTextShort})` : ` (${optionalText})`;
    this.renderer.appendChild(this.elementRef.nativeElement, child);
  }
}
