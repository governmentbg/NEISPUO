### Define scenarios 

1. Define scenarios in the performance.test.yaml
1. In the user_jwt_tokens.csv copy paste the JWTs you got after login through the browser
1. Update the URL in the performance.test.yml to reflect the environment against which you want to be running the tests
1. In order to run the performance execute ./node_modules/.bin/artillery run test/artillery-performance-test/performance.test.yml

### Additional documentation

- https://artillery.io/docs/guides/guides/test-script-reference.html