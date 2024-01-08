## Expression Generator For LINQ

## Sample JSON for rule

```json
{
  "condition": "and",
  "order" : {
	  "field" : "Name",
	  "sort" : "descending"
  },
  "rules": [
    {
      "field": "Name",
      "operator": "starts_with",
      "type": "string",
      "value": "M"
    },
	{
      "condition" : "and",
	  "rules" : [
	    {
		  "field": "Team.Department.Company.FoundedAtYear",
		  "operator": "in",
		  "type": "int",
		  "value": [
			1996,
			1999,
			2005,
			2003,
			2008,
			2013,
			2015
		  ]
	    }
	  ]
	},
	{
	  "field": "Team.Department.Title",
	  "operator": "contains",
	  "type": "string",
      "value": "-1"
	}
  ]
}
```

Inspired By https://github.com/JeremyLikness/ExpressionGenerator
