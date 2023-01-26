import {DateTime} from 'luxon';
import {GuidService} from '../../../../../modules/core/app/services/state-management/guid.service';
import {RestApi} from '../../../../fixtures/services/restApi';

describe('Create and edit', () => {
  before(() => {
    const testId = GuidService.newGuid().substring(0, 8);
    RestApi.createCustomer(testId).then(response => RestApi.createProject(testId, response.body.id));
    RestApi.createActivity(testId);
  })

  it('When a record is created, it can be saved', () => {
    cy.intercept('/api/v1/Typeahead/GetProjects').as('getProjects');
    cy.intercept('/api/v1/Typeahead/GetActivities').as('getActivities');

    cy.visit('/00000000-0000-0000-0000-000000000000');

    cy.wait('@getProjects');
    cy.wait('@getActivities');

    cy.get('#projectId').click().type('{enter}');
    cy.get('#activityId').click().type('{enter}');

    cy.get('#timesheetForm').as('timesheetForm');
    cy.get('@timesheetForm').should('exist');
    cy.intercept('http://localhost:5000/api/v1/TimeSheet/Create').as('getTimeSheet');
    cy.get('#save').click();

    cy.get('@timesheetForm').should('not.exist');
    cy.wait('@getTimeSheet').its('response.statusCode').should('eq', 200);
  })
})

describe('Date picker', () => {
  beforeEach(() => {
    cy.intercept('GET', '/api/v1/TimeSheet/Get/88dd9b02-9db0-4e0d-ce5f-08d9f7c722f1', {fixture: 'dto/TimeSheetDto.default.json'}).as('default');
  })

  it('When end date on new entry is set, time is set to \'now\'', () => {
    cy.visit('/00000000-0000-0000-0000-000000000000');

    cy.get('#endDate').type("{selectall}1");
    cy.get('#endTime').click();

    const now = DateTime.now();
    const oneMinuteAhead = now.plus({minute: 1});
    const expectedText = RegExp(`(${now.toFormat('HH:mm')}|${oneMinuteAhead.toFormat('HH:mm')})`);
    cy.get('#endTime').invoke('val').should('match', expectedText);
  })

  it('When end date on new entry is changed, time remains unchanged', () => {
    cy.visit('/00000000-0000-0000-0000-000000000000');

    cy.get('#endDate').type("{selectall}1");
    cy.get('#endTime').click().type('16:34');
    cy.get('#endDate').type("{selectall}2");
    cy.get('#endTime')
      .click()
      .should('have.value', '16:34');
  })

  it('When existing entry is loaded, start and end date are displayed unmodified', () => {
    cy.visit('/88dd9b02-9db0-4e0d-ce5f-08d9f7c722f1');

    cy.get('#startTime')
      .click()
      .should('have.value', '15:00');

    cy.get('#endTime')
      .click()
      .should('have.value', '20:00');
  })

  it('When end date on existing entry is changed, time remains unchanged', () => {
    cy.visit('/88dd9b02-9db0-4e0d-ce5f-08d9f7c722f1');

    cy.get('#endDate').type("{selectall}1");
    cy.get('#endTime')
      .click()
      .should('have.value', '20:00');
  })
})
