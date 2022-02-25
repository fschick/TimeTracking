import {DateTime} from 'luxon';

describe('Date picker', () => {
  beforeEach(() => {
    cy.intercept('GET', '/api/v1/Information/GetProductInformation', {fixture: 'dto/ProductInformationDto.default.json'}).as('default');
    cy.intercept('GET', '/api/v1/TimeSheet/GetListFiltered**', {fixture: 'generic/array.empty.json'}).as('empty');
    cy.intercept('GET', '/api/v1/Typeahead/GetCustomers', {fixture: 'generic/array.empty.json'}).as('empty');
    cy.intercept('GET', '/api/v1/Typeahead/GetProjects', {fixture: 'generic/array.empty.json'}).as('empty');
    cy.intercept('GET', '/api/v1/Typeahead/GetActivities', {fixture: 'generic/array.empty.json'}).as('empty');
    cy.intercept('GET', '/api/v1/Typeahead/GetOrders', {fixture: 'generic/array.empty.json'}).as('empty');
    cy.intercept('GET', '/api/v1/TimeSheet/Get/88dd9b02-9db0-4e0d-ce5f-08d9f7c722f1', {fixture: 'dto/TimeSheetDto.default.json'}).as('default');
  })

  it('When end date is set, time is set to \'now\'', () => {
    cy.visit('/00000000-0000-0000-0000-000000000000');

    const now = DateTime.now().toFormat('HH:mm');
    cy.get('#endDate').type("{selectall}1");
    cy.get('#endTime')
      .click()
      .should('have.value', now);
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
