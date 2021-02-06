import {UrlMatcher, UrlMatchResult, UrlSegment} from '@angular/router';
import {pathToRegexp} from 'path-to-regexp';

// eslint-disable-next-line prefer-arrow/prefer-arrow-functions
export function rematch(path: string): UrlMatcher {
  return (urlSegments: UrlSegment[]/*, group: UrlSegmentGroup, route: Route*/): UrlMatchResult | null => {
    const paramKeys: any = [];
    const pathRegex = pathToRegexp(path, paramKeys, {end: false});

    const url = urlSegments.map(x => x.path).join('/');
    const matches = pathRegex.exec(url);
    if (matches === null)
      return null;

    const parameters = matches.slice(1);
    const firstParameterUrlIndex = urlSegments.findIndex(x => x.path === parameters[0]);
    const parameterUrls = firstParameterUrlIndex >= 0
      ? urlSegments.slice(firstParameterUrlIndex)
      : [];

    const allParameterUrlsMatches = parameters.every((value, idx) => value === parameterUrls[idx]?.path);
    if (!allParameterUrlsMatches)
      return null;

    const consumedSegments = firstParameterUrlIndex >= 0
      ? urlSegments.slice(0, firstParameterUrlIndex + parameters.length)
      : urlSegments;

    const posParams = parameters
      .map((param, idx) => [paramKeys[idx].name, parameterUrls[idx]])
      .filter(x => x[1]?.path !== undefined);

    return {consumed: consumedSegments, posParams: Object.fromEntries(posParams)};
  };
}
