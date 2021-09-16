import {DurationPipe} from './duration.pipe';
import {TestBed} from '@angular/core/testing';
import {DateTime, Duration} from 'luxon';

describe('DurationPipe', () => {
  let pipe: DurationPipe;

  beforeEach(() => {
    TestBed.configureTestingModule({providers: [DurationPipe]});
    pipe = TestBed.inject(DurationPipe);
  });

  it('create an instance', () => {
    expect(pipe).toBeTruthy();
  });

  it('duration should be formatted to \'hh:mm\' as default', () => {
    const duration = Duration.fromObject({hours: 9, minutes: 20});
    const transformedDuration = pipe.transform(duration);
    const expectedDuration = '09:20';
    expect(transformedDuration).toBe(expectedDuration);
  });

  it('duration with days should be formatted to \'hh:mm\' as default', () => {
    const duration = Duration.fromObject({days: 1, hours: 8, minutes: 20});
    const transformedDuration = pipe.transform(duration);
    const expectedDuration = '32:20';
    expect(transformedDuration).toBe(expectedDuration);
  });

  it('duration should be formattable via custom pattern', () => {
    const duration = Duration.fromObject({days: 1, hours: 8, minutes: 20});
    const transformedDuration = pipe.transform(duration, 'd \'day\', hh:mm');
    const expectedDuration = '1 day, 08:20';
    expect(transformedDuration).toBe(expectedDuration);
  });
});
