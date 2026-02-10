### Define scenarios 

1. Define scenarios in the performance.test.yaml
1. In the user_jwt_tokens.csv copy paste the JWTs you got after login through the browser
1. In the interactions.csv copy and paste information after sucessful login as a user with multiple roles. Make sure the `selectedRole._concatID` is correct. If you want to log in with multiple users, use incognito to get the Cookies & interaction, so that the session is valid.
1. Update the URL in the performance.test.yml to reflect the environment against which you want to be running the tests. Don't forget to check the `client_id` and `redurect_uri` when connecting to the tenant.
1. In order to run the performance execute ./node_modules/.bin/artillery run test/artillery-performance-test/performance.test.yml

### Not included endpoints
1. /azure-integration-callback endpoint: Didn't find a way to programmatically generate auth code. If you try to manually log in the tenant (azure) it gives you a code, but instantly uses it which makes it invalid. And the endpoint will always return error because we cannot get a fresh auth code.
1. /interaction/:uid/successful_login endpoint: Because it ends the interaction and can only be called once per interaction. Because of 1. we cannot login programmatically to create custom interactions.

### Additional documentation

- https://artillery.io/docs/guides/guides/test-script-reference.html

### DEBUG

- You can use env `DEBUG=http,http:capture,http:response`
- For Git Bash the command looks like: `export DEBUG=http,http:capture,http:response && artillery run test/artillery-performance-test/performance.test.yml`