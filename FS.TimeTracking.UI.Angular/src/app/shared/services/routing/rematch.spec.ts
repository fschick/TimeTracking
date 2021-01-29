/* eslint-disable prefer-arrow/prefer-arrow-functions */
import {rematch} from './rematch';
import {Route, UrlMatchResult, UrlSegment, UrlSegmentGroup} from '@angular/router';

describe('rematch', () => {
  it('should match simple path', () => {
    const matcher = rematch('master-data');

    const match = matcher(toSegments('master-data'), {} as UrlSegmentGroup, {} as Route);
    const expected = {consumed: [{path: 'master-data'}], posParams: {}} as UrlMatchResult;
    expect(match).toEqual(expected);

    const noMatch = matcher(toSegments('reports'), {} as UrlSegmentGroup, {} as Route);
    expect(noMatch).toBeNull();
  });

  it('should match path with parameters', () => {
    const matcher = rematch('master-data/:entity/:id');

    const noParameter = matcher(toSegments('master-data'), {} as UrlSegmentGroup, {} as Route);
    expect(noParameter).toBeNull();

    const oneParameter = matcher(toSegments('master-data/customers'), {} as UrlSegmentGroup, {} as Route);
    expect(oneParameter).toBeNull();

    const twoParameter = matcher(toSegments('master-data/customers/11'), {} as UrlSegmentGroup, {} as Route);
    const expected = {
      consumed: [{path: 'master-data'}, {path: 'customers'}, {path: '11'}],
      posParams: {entity: {path: 'customers'}, id: {path: '11'}}
    } as unknown as UrlMatchResult;
    expect(twoParameter).toEqual(expected);

    const threeParameter = matcher(toSegments('master-data/customers/11/create'), {} as UrlSegmentGroup, {} as Route);
    expect(threeParameter).toBeNull();
  });

  it('should match path with optional parameters', () => {
    const matcher = rematch('master-data/:entity?/:id?');

    const noParameter = matcher(toSegments('master-data'), {} as UrlSegmentGroup, {} as Route);
    const noExpected = {
      consumed: [{path: 'master-data'}],
      posParams: {}
    } as unknown as UrlMatchResult;
    expect(noParameter).toEqual(noExpected);

    const oneParameter = matcher(toSegments('master-data/customers'), {} as UrlSegmentGroup, {} as Route);
    const oneExpected = {
      consumed: [{path: 'master-data'}, {path: 'customers'}],
      posParams: {entity: {path: 'customers'}}
    } as unknown as UrlMatchResult;
    expect(oneParameter).toEqual(oneExpected);

    const twoParameter = matcher(toSegments('master-data/customers/11'), {} as UrlSegmentGroup, {} as Route);
    const twoExpected = {
      consumed: [{path: 'master-data'}, {path: 'customers'}, {path: '11'}],
      posParams: {entity: {path: 'customers'}, id: {path: '11'}}
    } as unknown as UrlMatchResult;
    expect(twoParameter).toEqual(twoExpected);

    const threeParameter = matcher(toSegments('master-data/customers/11/create'), {} as UrlSegmentGroup, {} as Route);
    expect(threeParameter).toBeNull();
  });

  it('should match path with custom parameters', () => {
    const matcher = rematch('master-data/:entity(customers|projects|activities)?/:id?');

    const match = matcher(toSegments('master-data/customers/11'), {} as UrlSegmentGroup, {} as Route);
    const expected = {
      consumed: [{path: 'master-data'}, {path: 'customers'}, {path: '11'}],
      posParams: {entity: {path: 'customers'}, id: {path: '11'}}
    } as unknown as UrlMatchResult;
    expect(match).toEqual(expected);

    const noMatch = matcher(toSegments('master-data/reports/11'), {} as UrlSegmentGroup, {} as Route);
    expect(noMatch).toBeNull();
  });

  function toSegments(path: string): UrlSegment[] {
    return path.split('/').map(x => ({path: x})) as UrlSegment[];
  }
});
