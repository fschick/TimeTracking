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

  it('should match nested path', () => {
    const matcher = rematch('admin/master-data');

    const match = matcher(toSegments('admin/master-data'), {} as UrlSegmentGroup, {} as Route);
    const expected = {consumed: [{path: 'admin'}, {path: 'master-data'}], posParams: {}} as UrlMatchResult;
    expect(match).toEqual(expected);

    const noMatch = matcher(toSegments('admin/reports'), {} as UrlSegmentGroup, {} as Route);
    expect(noMatch).toBeNull();
  });

  it('should match path with parameters', () => {
    const matcher = rematch('admin/master-data/:entity/:id');

    const noParameter = matcher(toSegments('admin/master-data'), {} as UrlSegmentGroup, {} as Route);
    expect(noParameter).toBeNull();

    const oneParameter = matcher(toSegments('admin/master-data/customers'), {} as UrlSegmentGroup, {} as Route);
    expect(oneParameter).toBeNull();

    const twoParameter = matcher(toSegments('admin/master-data/customers/11'), {} as UrlSegmentGroup, {} as Route);
    const expected = {
      consumed: [{path: 'admin'}, {path: 'master-data'}, {path: 'customers'}, {path: '11'}],
      posParams: {entity: {path: 'customers'}, id: {path: '11'}}
    } as unknown as UrlMatchResult;
    expect(twoParameter).toEqual(expected);

    const threeParameter = matcher(toSegments('admin/master-data/customers/11/create'), {} as UrlSegmentGroup, {} as Route);
    const threeExpected = {
      consumed: [{path: 'admin'}, {path: 'master-data'}, {path: 'customers'}, {path: '11'}],
      posParams: {entity: {path: 'customers'}, id: {path: '11'}}
    } as unknown as UrlMatchResult;
    expect(threeParameter).toEqual(threeExpected);
  });

  it('should match path with optional parameters', () => {
    const matcher = rematch('admin/master-data/:entity?/:id?');

    const noParameter = matcher(toSegments('admin/master-data'), {} as UrlSegmentGroup, {} as Route);
    const noExpected = {
      consumed: [{path: 'admin'}, {path: 'master-data'}],
      posParams: {}
    } as unknown as UrlMatchResult;
    expect(noParameter).toEqual(noExpected);

    const oneParameter = matcher(toSegments('admin/master-data/customers'), {} as UrlSegmentGroup, {} as Route);
    const oneExpected = {
      consumed: [{path: 'admin'}, {path: 'master-data'}, {path: 'customers'}],
      posParams: {entity: {path: 'customers'}}
    } as unknown as UrlMatchResult;
    expect(oneParameter).toEqual(oneExpected);

    const twoParameter = matcher(toSegments('admin/master-data/customers/11'), {} as UrlSegmentGroup, {} as Route);
    const twoExpected = {
      consumed: [{path: 'admin'}, {path: 'master-data'}, {path: 'customers'}, {path: '11'}],
      posParams: {entity: {path: 'customers'}, id: {path: '11'}}
    } as unknown as UrlMatchResult;
    expect(twoParameter).toEqual(twoExpected);

    const threeParameter = matcher(toSegments('admin/master-data/customers/11/create'), {} as UrlSegmentGroup, {} as Route);
    const threeExpected = {
      consumed: [{path: 'admin'}, {path: 'master-data'}, {path: 'customers'}, {path: '11'}],
      posParams: {entity: {path: 'customers'}, id: {path: '11'}}
    } as unknown as UrlMatchResult;
    expect(threeParameter).toEqual(threeExpected);
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

  it('should match path with regex parameters', () => {
    const matcher = rematch('master-data/:id([a-zA-Z0-9]{8}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{12})');

    const match = matcher(toSegments('master-data/b4dd4db3-5d27-4fe4-8993-48e742577fff'), {} as UrlSegmentGroup, {} as Route);
    const expected = {
      consumed: [{path: 'master-data'}, {path: 'b4dd4db3-5d27-4fe4-8993-48e742577fff'}],
      posParams: {id: {path: 'b4dd4db3-5d27-4fe4-8993-48e742577fff'}}
    } as unknown as UrlMatchResult;
    expect(match).toEqual(expected);

    const noMatch = matcher(toSegments('master-data/subpage'), {} as UrlSegmentGroup, {} as Route);
    expect(noMatch).toBeNull();
  });

  function toSegments(path: string): UrlSegment[] {
    return path.split('/').map(x => ({path: x})) as UrlSegment[];
  }
});
