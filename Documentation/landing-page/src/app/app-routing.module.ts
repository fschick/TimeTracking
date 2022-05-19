import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {OverviewComponent} from './components/overview/overview.component';

const routes: Routes = [{
  path: '', component: OverviewComponent,
},];

@NgModule({
  imports: [RouterModule.forRoot(routes, {
    scrollPositionRestoration: 'enabled',
    anchorScrolling: 'enabled',
    scrollOffset: [0, 60], // [x, y]
    onSameUrlNavigation: 'reload', // https://stackoverflow.com/questions/50836497/using-html-anchor-link-id-in-angular-6#comment102332899_52724769
  })],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
