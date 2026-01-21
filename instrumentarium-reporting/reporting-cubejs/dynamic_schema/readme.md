# Deprecation notes

The reason why the dynamic schema is not currently being used is that drillMembers are not properly processed and displayed when fetching schemas therefore the functionality for drills is not working correctly.

Ideally you could be able to adress the issue by modifying the functions parsing the data in `utils.js`
