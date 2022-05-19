import {Component, OnInit} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {single} from 'rxjs';

type ReleaseInfo = {
  tag_name: string;
  assets: {
    name: string;
    browser_download_url: string;
  }[]
};

@Component({
  selector: 'app-overview',
  templateUrl: './overview.component.html',
  styleUrls: ['./overview.component.scss']
})
export class OverviewComponent implements OnInit {

  public downloadUrlWindows?: string;
  public downloadUrlLinux?: string;
  public latestVersion?: string;
  public os: 'windows' | 'linux' = 'windows';

  constructor(
    httpClient: HttpClient
  ) {
    const releaseApiUrl = 'https://api.github.com/repos/fschick/TimeTracking/releases/latest';
    httpClient.get<ReleaseInfo>(releaseApiUrl)
      .pipe(single())
      .subscribe(releaseInfo => {
        this.latestVersion = releaseInfo.tag_name;
        this.downloadUrlWindows = releaseInfo.assets.find(asset => asset.name.includes('windows'))?.browser_download_url;
        this.downloadUrlLinux = releaseInfo.assets.find(asset => asset.name.includes('linux'))?.browser_download_url;
      });
  }

  ngOnInit(): void {
  }

}
