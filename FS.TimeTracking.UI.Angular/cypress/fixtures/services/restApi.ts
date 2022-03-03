import Chainable = Cypress.Chainable;
import Response = Cypress.Response;

export class RestApi {
  public static createCustomer(testId: string): Chainable<Response<any>> {
    return cy.request('POST', 'http://localhost:5000/api/v1/Customer/Create', {
      "title": `Customer-${testId}`
    });
  }

  public static createProject(testId: string, customerId: number): Chainable<Response<any>> {
    return cy.request('POST', 'http://localhost:5000/api/v1/Project/Create', {
      "title": `Project-${testId}`,
      "customerId": customerId
    });
  }

  public static createActivity(testId: string): Chainable<Response<any>> {
    return cy.request('POST', 'http://localhost:5000/api/v1/Activity/Create', {
      "title": `Activity-${testId}`
    });
  }
}
