import {UrlMatcher, UrlMatchResult, UrlSegment} from '@angular/router';
import {pathToRegexp} from 'path-to-regexp';

// eslint-disable-next-line prefer-arrow/prefer-arrow-functions
export function rematch(path: string): UrlMatcher {
  return (urlSegments: UrlSegment[]/*, group: UrlSegmentGroup, route: Route*/): UrlMatchResult | null => {
    const paramKeys: any = [];
    const pathRegex = pathToRegexp(path, paramKeys);

    const url = urlSegments.map(x => x.path).join('/');
    const matches = pathRegex.exec(url);
    if (matches === null)
      return null;

    const posParams: { [name: string]: UrlSegment } = {};
    const params = matches.filter(param => param !== undefined);
    for (let idx = 1; idx < params.length; idx++) {
      posParams[paramKeys[idx - 1].name] = urlSegments[idx];
    }

    return {consumed: urlSegments, posParams};
  };
}
