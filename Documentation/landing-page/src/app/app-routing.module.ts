import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {MainPageComponent} from './components/overview/main-page/main-page.component';

const routes: Routes = [{
  path: '', component: MainPageComponent,
}];

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
