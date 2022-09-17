
# Jira Sender

An additional sender for REPORTS.

## Setup

### Issue URL
This is the API route for adding an issue.  At time of writing, the following works best.
```http
https://<your jira instance>/rest/api/3/issue
```

### Image URL
This is the API route for adding images to the issue.
```http
https://<your jira instance>/rest/api/3/issue/{0}/attachments
```

### Jira Email
This is the email for the account you wish to use for Jira.  I recommend making an account specifically for this purpose, it will make it easier to see which issues are user reports.

### Jira Api Token
This is your generated API Token from your Jira account.

[(Jira) Manage API tokens for your Atlassian account](https://support.atlassian.com/atlassian-account/docs/manage-api-tokens-for-your-atlassian-account/)


### Jira Project Key

This is the key for your Jira project. The Project Key is the prefix of the issue number.  For example: `JRP-123`, the "`JRP`" part of this is the Project Key.

### Issue Types
The Issue Types type is used for assigning the correct issue type in Jira based on the report type the user selects in REPORTS.

In the Jira Sender component you should add each issue type you wish to use, making sure the **name** and **ID** are exactly correct.

**name:** This should be the same as one of the Report Types specified in the [REPORTS Widget Configuration](https://github.com/emanzione/REPORTS/wiki/Getting-Started#basic-configuration).


**ID:** This should respond to the corresponding Jira issue ID.  To find the ID for your issue types, see the following.

[(Confluence) Finding the ID for Issue Types](https://confluence.atlassian.com/jirakb/finding-the-id-for-issue-types-646186508.html)

* You do not need every report type specified in `Widget Configuration` here, you only need one.  If the Jira sender cannot match the chosen report type, it will use the first issue in the Issue Types list.

### Jira Json

This is the data structure the sender will use to send data to the Jira API.  You can change this, but you will have to add to the JiraSender script and modify the `JToken json` field. 

Use the below to get up and running straight away.

```json
{
    "fields": {
        "summary": "Default Summary",
        "issuetype": {
            "id": "10004"
        },
        "project": {
            "key": "TEST"
        },
        "description": {
            "type": "doc",
            "version": 1,
            "content": [
                {
                "type": "paragraph",
                "content": [
                    {
                    "text": "This is the description.",
                    "type": "text"
                    }
                ]
                }
            ]
        }
    }
}

```

## Common Issues
### `issuetype: Specify a valid issue type`

Are your Issue IDs correct, have you got the correct issue ID from Jira?