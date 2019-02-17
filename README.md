# Sir.Resin

## What is this?

A non-tracking [boolean search](https://didyougogo.com) engine.

### Create your own collection

Go ahead and:  

#### POST a JSON document to the WRITE endpoint

	HTTPS POST didyougogo.com/io/[collection_name]
	Content-Type:application/json
	[
		{
			"field1":"value1"
		},
		{
			"field1":"value2"
		}
	]

	Server should respond with a list of document IDs:
	[
		1,
		2
	]

### GET document by ID

	HTTPS GET didyougogo.com/io/[collection_name]?id=[document_id]
	Accept:application/json

### Query collection through the API

	HTTPS GET didyougogo.com/io/[collection_name]?q=[phrase-or-term-query]&fields=title&skip=0&take=10
	Accept:application/json

### Human-friendly query GUI

	HTTPS GET didyougogo.com/?q=[phrase-or-term-query]&fields=title&skip=0&take=10&collection=[collection_name]

### Read more

#### HTTP reader/writer micro-service framework.
Create distributable readers and writer.
https://github.com/kreeben/resin/tree/master/src/Sir.HttpServer

#### A Int64/Int64[] key/value writer service and queryable map/reduce reader. 
Execute set operations over local lists of Int64's (document references).  
https://github.com/kreeben/resin/tree/master/src/Sir.Postings

#### Document writer and queryable map/reduce orchestrator. 
Orchestrate set operations over remote lists of document references.   
https://github.com/kreeben/resin/tree/master/src/Sir.Store

### Roadmap

- [x] v0.1a - bag-of-characters term vector space language model
- [x] v0.2a - HTTP API comprised of distributable search microservices
- [x] v0.3a - boolean query language with support for AND ('+'), OR (' '), NOT ('-') and scope ('(', ')').
- [ ] v0.4a - bag-of-words document vector space language model
- [ ] v0.5b - semantic language model
- [ ] v0.6b - local join between collections
- [ ] v0.7b - distributed join between collections
- [ ] v0.8 - voice-to-text
- [ ] v0.9 - image search
- [ ] v1.0 - text-to-voice
- [ ] v2.0 - AI
