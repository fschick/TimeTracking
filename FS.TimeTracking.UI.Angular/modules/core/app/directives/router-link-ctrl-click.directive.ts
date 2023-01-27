import {Directive} from '@angular/core';
import {RouterLink} from '@angular/router';

// https://github.com/flathub/linux-store-frontend/issues/4#issuecomment-812113202
@Directive({
  // eslint-disable-next-line @angular-eslint/directive-selector
  selector: 'a[routerLink],area[routerLink]'
})
export class RouterLinkCtrlClickDirective extends RouterLink {
  public override onClick(button: number, ctrlKey: boolean): boolean {
    if (this.urlTree && ctrlKey && button === 0)
      window.open(this.urlTree.toString(), '_blank');

    return true;
  }
}
