import {Component, Input} from '@angular/core';

@Component({
  selector: 'ts-icon',
  templateUrl: './icon.component.html',
  styleUrls: ['./icon.component.scss']
})
export class IconComponent {
  @Input() icon = '';
}
