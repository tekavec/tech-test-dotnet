## ClearBank.DeveloperTest - Code Analysis

After initial code observation, I found these problems with the code and potential improvements:

1. `MakePayment` method breaks several SOLID principles, namely:
  * Single Responsibility Principle: creating data store, retrieving account, validating account state, updating account balance.
  * Open Closed Principle: if in the future there are more data stores that must be used, or new payment schemes, we'd have to modify `MakePayment` method, whereas a better approach would be to just extend functionality, for example by adding new classes.
  * Dependency Inversion Principle: there are several instantiations of classes this method depend on directly implemented in the method; this makes unit testing very hard and creates a tight coupling between the `MakePayment` method and its dependencies. A better solution would be to depend only on abstractions - injecting dependencies through the `PaymentService` constructor. This would be beneficial for both production code and unit testing, as we'd be in control which implementation of abstraction (e.g. interface) is suitable for different purpose.
  
2. Code in the `MakePayment` method is relatively complex and hard to follow:
  * Reader must battle different levels of abstractions - for example, there are calls to high-level methods of other types, but also low level parts of the code which deal with balance calculations or checking for the supported payment scheme.
  * Cyclomatic complexity: just from observing the code, it's quite hard to understand what the combinations of `switch` and `if-else` statements do
  * Code smells: business logic duplication (e.g. checking twice the type of data store, as well as creating data store twice), too many local variables with default initial value, 'magic' values (e.g. "Backup"), primitive obsession (Amount deserves its own type, not just decimal value - currency should be part of it)
  * Lack of currency might be a problem on its own - even if Account in this system is a single-currency concept, it'd be better to have currency explicitly stated and somewhere in the payment workflow currency compatibility should be checked.
  * Specific coupling to application settings - method uses an obsolete approach (Options would be a better one), which forces the method to know about the internal structure of the app. settings, and it's hard to maintain as there's no type safety in case app. setting key changes - in such case, code shouldn't break when the method is called, but in the app initialisation stage.

3. Domain specifics
  * Between the account creation, checking its balance and updating it, there's a chance its state can change - either in multi-threading system or database updates from another system. In a real application, I'd guard against such problems with one of the locking features (optimistic/pessimistic, or thread locking) or going with immutable event sourcing approach if response doesn't have to be quick. But I'm deliberately leaving this outside the scope of this exercise (time constraint, not having visibility of the whole system).
  * `PaymentScheme` enumerator doesn't have assigned integer values to its members, which means it defaults to `FasterPayments`. This could be potentially dangerous as forgetting to set payment scheme in the payment request might go through the unexpected rail. However, I wouldn't change enumerator values if solution already went to production as it'd cause wrong resolution of stored payment scheme values.

4. Project level:
  * Good separation of production code and tests on the project level.
  * production code does have some folder hierarchy - and this is somehow dependent on personal taste or company's guidelines, but I usually navigate through the structure easier if it's split by domain concepts (e.g. payment, account, store...), not code categories ("Type" is especially problematic, but "Services" will likely grow too and require additional hierarchy levels).
  
And, of course, there are no unit tests.

## Refactoring plan

After initial read and code analysis, I plan to perform the following steps:

#### 1. Cover all execution paths of the `MakePayment` method with unit tests.

This is a critical step as I can see the method cannot be unit tested without modification - hence, the only modification I'll allow to myself, will be automated refactorings provided by IDE. 
Initially, I'll have to unit test an inherited implementation of the `PaymentService` class, which will replace hardcoded dependencies with controlled values from my unit tests. But the logic of the `MakePayment` method will be fully tested nevertheless (and this temp class can be later removed once I'm able to inject dependencies).

I'll be using code coverage tool NCrunch (https://ncrunch.net/), which shows live code coverage and test success. This will both speed up code coverage but it also makes easier to understand whether I covered all execution paths.

I'll use xunit and Moq libraries in my test project. I expect to have a mix of 'Fact' unit tests and 'Theory' unit tests for permutations or edge cases when this will be beneficial to reduce the number of unit tests.
Unit test implementation will generally be split in 3 sections - Arrange, Act, and Assert. I don't write comments above each section, they are split by an empty line only.

**Note:** I will deliberately break the convention on C# method naming for unit test methods - adding underscore between words for better readability. I prefer to have unit tests write in a way that class name + method name can be read as sentences, for example: "Payment service should not update an account if account not found" translates to "PaymentServiceShould" (class name) and "Not Update_Account_If_Account_Not_Found" (method name).

#### 2. Refactoring

Once I have full test coverage (apart from hardcoded external dependencies) and permutations, the party begins - I'll be free to make modifications to the `MakePayment` method and get immediate indication if I broke something. I'll try to stay in the 'green' zone a much as possible, which means I'll commit more often too.

I'll almost certainly start with dependency injection to simplify unit tests, creating new types to delegate some responsibilities outside the `MakePayment` method. For each new type I'll introduce, I'll create unit tests first, and then implement functionality step-by-step.

I expect to see much simplified code in the `MakePayment` method, which should create additional opportunities for simplifying the code.


#### 3. Final cleanup

Final implementation and test code cleanup - only what was forgotten during the refactoring but will try to be diligent already as I progress.

