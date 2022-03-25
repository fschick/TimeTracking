describe('TimeSheet table', () => {
  beforeEach(() => {
    cy.intercept('GET', '/api/v1/TimeSheet/GetGridFiltered**', {fixture: 'dto/TimeSheetGridDto.single.json'}).as('default');
  })

  it('Caption is displayed with matching spaces (no holidays)', () => {
    cy.intercept('GET', '/api/v1/Workday/GetWorkedDaysInfo**', {fixture: 'dto/WorkedTimeInfoDto.single.json'}).as('default');
    cy.visit('/');

    cy.get('#timeSheetTableCaption')
      .should('have.text', "Timesheet (0.8 days worked,  1 working days) ");
  })

  it('Caption is displayed with matching spaces (with holidays)', () => {
    cy.intercept('GET', '/api/v1/Workday/GetWorkedDaysInfo**', {fixture: 'dto/WorkedTimeInfoDto.holidays.json'}).as('default');
    cy.visit('/');

    cy.get('#timeSheetTableCaption')
      .should('have.text', "Timesheet (0.8 days worked,  1 working days,  1 vacation days) ");
  })
})
