describe('Table', () => {
  beforeEach(() => {
    cy.intercept('GET', '/api/v1/TimeSheet/GetListFiltered**', {fixture: 'dto/TimeSheetListDto.single.json'}).as('default');
    cy.intercept('GET', '/api/v1/Workday/GetWorkedDaysInfo**', {fixture: 'dto/WorkedTimeInfoDto.single.json'}).as('default');
  })

  it('Caption is displayed with mathcing spaces', () => {
    cy.visit('/');

    cy.get('#timeSheetTableCaption')
      .should('have.text', "Timesheet (0.8 days worked,  1 workdays,  0 holidays) ");
  })
})
