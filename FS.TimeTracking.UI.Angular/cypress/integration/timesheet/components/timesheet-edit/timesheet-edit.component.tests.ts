﻿import {DateTime} from 'luxon';

describe('Date picker', () => {
  beforeEach(() => {
    cy.intercept('GET', '/api/v1/TimeSheet/Get/88dd9b02-9db0-4e0d-ce5f-08d9f7c722f1', {fixture: 'dto/TimeSheetDto.default.json'}).as('default');
  })

  it('When end date is set, time is set to \'now\'', () => {
    cy.visit('/00000000-0000-0000-0000-000000000000');

    cy.get('#endDate').type("{selectall}1");
    cy.get('#endTime').click();

    const now = DateTime.now();
    const oneMinuteAhead = now.plus({minute: 1});
    const expectedText = RegExp(`(${now.toFormat('HH:mm')}|${oneMinuteAhead.toFormat('HH:mm')})`);
    cy.get('#endTime').invoke('val').should('match', expectedText);
  })

  it('When end date is changed, time remains unchanged', () => {
    cy.visit('/00000000-0000-0000-0000-000000000000');

    const now = DateTime.now().toFormat('HH:mm');
    cy.get('#endDate').type("{selectall}1");
    cy.get('#endTime').click().type('16:34');
    cy.get('#endDate').type("{selectall}2");
    cy.get('#endTime')
      .click()
      .should('have.value', '16:34');
  })

  it('Should display existing entry unmodified after open', () => {
    cy.visit('/88dd9b02-9db0-4e0d-ce5f-08d9f7c722f1');

    cy.get('#startTime')
      .click()
      .should('have.value', '15:00');

    cy.get('#endTime')
      .click()
      .should('have.value', '20:00');
  })
})
