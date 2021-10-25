Course of RabbitMQ to learn core concepts

1. Fanout exchange

* Simplest exchange type, it sends all the messages it receives to all the queues that are bound to it.
* It simply ignores the routing information and does not perform any filtering
* Like a postman that photocopies all the mails and puts one copy into each mailbox


2. Direct Exchange

* Routes messages to the queues based on the "routing key" specified in binding definition.
* In order to send a message to a queue, routing key on the message and the routing key of the bound queue must be exactly the same.

3. Topic Exchange

* Uses routing key in order to route a message, but does not require full match, checks the pattern instead.
* Checks whether the routing key "pattern" of a queue matches the received message's routing key.
* Routing key may include more than one word, separated by dots.
* Routing key of a queue can contain wild cards for a matching message's routing key.
* Available wild cards are as follows:
    *(Asterisk): Matches exactly one word.
        "*.image" will match "convert.image", but not "convert.bitmap.image" or "image.jpg".
    #(hash): matches zero or more words.
    "images.#" will match "image.jpg" and "image.bitmap.32bit" but not "convert.image"

4. Headers Exchange

* Uses message headers in order to route a message to the bound queues.
* Ignores the routing key value of the message.
* A message may have many different headers with different values.
* While binding to this type of exchange, every queue specifies which headers a message may contain and wheter it requires "all" or "any" of them to be exist in the message.
* "x-match" is the special header key whose value can be "all" or "any". It determines the "match all" or "match any" logic for the matching process.
* Sample queue configurations.

5. Default Exchange

* When a new queue is created on a RabbitMQ system, it is implicitly bound to a system exchange called "default exchange", with a routing key which is the same as the queue name.
* Default exchange has no name (empty string).
* The type of default exchange is "direct".
* When sending a message, if exchange name is left empty, it is handled be the "default exchange".

6. Exchange to Exchange Binding

* Like binding a queue to an exchange, it is possible to bind an exchange to another exchange.
* Binding and message routing rules are the same.
* When an exchange is bound to another exchange, messages from the source exchange are routed to the destination exchange using the binding configuration.
* Finally, destination exchange routes these messages to its bound queues.

7. Alternate Exchange

* Some of the messages that are published to an exchange may not be suitable to route to any of the bound queues.
* These are unrouted messages.
* They are discarded by the exchange, so they are lost.
* In order to collect these messages, an "alternate exchange" can be defined for any exchange.
* Alternate exchange can be defined by setting the "alternate-exchange" key for and exchange.
* Any unrouted message is finally sent to defined "alternated-exchange".
* Any existing exchnage can be set as an "alternate exchange" for another exchange.
* Fanout exchanges, which do not perform any filtering, are good for using as an "alternate exchange".

CONSUMER PATTERN 

----

THERE ARE TWO WAYS TO GET MESSAGE FROM A QUEUE (PULL 'N PUSH)

1. Push Model

* Consumer application subscribes to the queue and waits for messages.
* If there is already a message on the queue.
* Or when a new message arrives, it's automatically sent(pushed) to the consumer application.
* This is the suggested way of getting messages from a queue.

2. Pull Model

* Consumer application does not subscribe to the queue.
* But it constantly checks (polls) the queue for new messages.
* If there's a message available on the queue, it's manually fetched (pulled) by the consumer application.
* Even though the pull mode is not recommended, it's the only solution when there is not live connection between message broker and consumer applications.

---
WORK/TASK QUEUES (COMPETING CONSUMERS PATTERN)

* Work queues are used to distributed task among multiple workers.
* Producers add tasks to a queue and these tasks are distributed to multiple worker applications.
* Pull or push models can be used to distribute tasks among the workers.
    * Pull Model: Workers get a message from the queue when they are available to perform a task
    * Push Model: Message broker system sends (pushes) messages to the available workers automatically.

---
PUBLISH SUBSCRIBE

* Publish-subscribe pattern is used to deliver the same message to all the subscribers.
* There may be one or more subscribers.
* In publish-subscribe pattern, there is one separate queue for each subscriber.
* Messages is delivered to each of these queues.
* So, each subscriber gets a copy of the same message.
* This pattern is mostly used for publishing event notifications.

---
REQUEST-REPLY / RESPONSE PATTERN

* Request-reply or request-response pattern is used when the publisher of the message, which is called requestor, needs to get the response for its message.
* The request message mostly contains a query or command.
* Using request-reply pattern, remote procedure call (RPC) scenarios can also be implemented.
* In request-reply patterns, there are at least two queues.
    * 1 for the request
    * 1 for the replies or responses. this queue is also named as the callback queue for RPC scenarios.