semantic-pipes
==============
Semantic Pipes is inspired by the UNIX philosophy for stitching multiple programs to build newly morphed programs. This library helps abstract the glue away from applications which uses the [Ports-And-Adapters](http://stackoverflow.com/questions/23081105/ports-and-adapters-hexagonal-architecture-clarification-of-terms-and-impleme) pattern.

The Ports and Adapters enroll their specific role by calling the `SemanticBuilder.InstallPipe` method. This method will allow the enrollment of both synchronous and asynchronous code with relative ease. When registrations occur, `semantic-pipes` will look for opportunities to build more complex "programs" under the hood (please see the Registry Observer Features below).

On the external facing applications, they will make a call to `ISemanticBroker.On<TInput>(input).Output<TOutput>()`. Based on the programs defined by the various enrolled ports and adapters, the various programs are then executed in the shortest path possible to achieve the `TOutput` desired.

NuGet
-----
	Install-Package SemanticPipes

Example
-------
Here is a quick start guide in a form of a unit test. The code is written to be read from top down, and the comments describes what is being demonstrated.

Take note that the test will run in about ±1 second, and not n*1sec where n=7 (parallel execution). This is because when the view model pipe scatters the various domain requests as a parallel task. Upon the gather of the `List<PingServerDomainResponse>`, an implicit gather is made. So how about that? Implied parallel processing loops without the programmer thinking about it.

    [TestFixture]
    public class QuickStart
    {
        /*
         *  These are the typical number of domains which exist
         *  within your DDD application for a certain command
         */

        public class PingServerCommandViewModel
        {
            public IEnumerable<int> Servers { get; set; }
        }

        public class PingServerResponseViewModel
        {
            public int NumberOfOnlineServers { get; set; }
            public int NumberOfOfflineServers { get; set; }
            public IEnumerable<int> OnlineServerInstanceIds { get; set; }
            public IEnumerable<int> OfflineServerInstanceIds { get; set; }
        }

        public class PingServerDomainCommand
        {
            public int ServerInstanceId { get; set; }
        }

        public class PingServerDomainResponse
        {
            public int ServerInstanceId { get; set; }
            public bool IsOnline { get; set; }
        }

        /*
         * So the incoming request and response are within the application edge.
         * 
         * The broker parameter is injected via a DI framework for the UI components,
         * however, there is nothing stopping you from moving the instance access elsewhere.
         * 
         * Also, take note of the Task being returned. Most modern application endpoints support
         * this kind of notation to assist in the number of concurrent requests that it would
         * be able to support. This is baked in
         */

        public Task<PingServerResponseViewModel> PingServers
            (ISemanticBroker broker, PingServerCommandViewModel command)
        {
            return broker.On(command).Output<PingServerResponseViewModel>();
        }

        /*
         * Imagine that this is your bootstraping components and creates
         * a global variable (or enrolls it into a DI framework).
         * 
         * For this example, we are going to simulate a request in the method
         */

        [Test]
        public async Task BoostrappingStartup()
        {
            var builder = new SemanticBuilder();

            // this is where you will scan all the relevant components into
            // the SemanticBuilder
            RegisterPipeComponents(builder);

            // after that, capture this instance (thread-safe) somewhere so
            // that you are able to reuse it with other requests
            ISemanticBroker broker = builder.CreateBroker();

            // simulate a request/response
            PingServerResponseViewModel response = await SimulateRequestResponse(broker);

            Assert.AreEqual(3, response.NumberOfOfflineServers);
            Assert.AreEqual(4, response.NumberOfOnlineServers);
        }

        private Task<PingServerResponseViewModel> SimulateRequestResponse(ISemanticBroker broker)
        {
            var requestModel = new PingServerCommandViewModel
            {
                Servers = new[] {1, 4, 7, 10, 11, 12, 20}
            };

            return PingServers(broker, requestModel);
        }

        private void RegisterPipeComponents(SemanticBuilder builder)
        {
            // logically, in your application, the domain pipes and the
            // view model pipes will be scattered around the solution
            RegisterViewModelComponents(builder);
            RegisterDomainModelComponents(builder);
        }

        private static void RegisterViewModelComponents(SemanticBuilder builder)
        {
            // firstly, our domain model doesn't support bulk queries natively,
            // so we are going to send them multiple single commands (see the IEnumerable)
            builder.InstallPipe<PingServerCommandViewModel, IEnumerable<PingServerDomainCommand>>(
                (model, broker) =>
                    model.Servers.Select(i => new PingServerDomainCommand {ServerInstanceId = i})
                );

            // since we know that we scatter the requests out, let's gather them in again
            // by inverting the IEnumerable (or this case, List - it doesn't matter)
            builder.InstallPipe<List<PingServerDomainResponse>, PingServerResponseViewModel>
                ((responses, broker) => new PingServerResponseViewModel()
                {
                    NumberOfOfflineServers = responses.Count(x => !x.IsOnline),
                    NumberOfOnlineServers = responses.Count(x => x.IsOnline),
                    OfflineServerInstanceIds =
                        responses.Where(x => !x.IsOnline).Select(x => x.ServerInstanceId),
                    OnlineServerInstanceIds =
                        responses.Where(x => x.IsOnline).Select(x => x.ServerInstanceId)
                });
        }

        private static void RegisterDomainModelComponents(SemanticBuilder builder)
        {
            // this is how the domain requests will come in, one by one.
            // notice that this pipe is executed asynchronously with the async keyword
            // being the compiler clue
            builder.InstallPipe<PingServerDomainCommand, PingServerDomainResponse>(
                async (command, broker) =>
                {
                    // imagine this is a database query or actual ping.
                    await Task.Delay(TimeSpan.FromSeconds(1));

                    return new PingServerDomainResponse()
                    {
                        // spot the odd one out :)
                        IsOnline = command.ServerInstanceId%2 == 0,
                        ServerInstanceId = command.ServerInstanceId
                    };
                });
        }
    }



Support
-------
Please feel free to contact me privately with your support questions, feature requests, and bug discoveries at [justin@justjuzzy.com](mailto:justin@justjuzzy.com?subject=[semantic-pipes]).

The Quick Documentation
=======================
Releasing open source code is a thankless job. If you have any questions, please drop me a mail at [justin@justjuzzy.com](mailto:justin@justjuzzy.com?subject=[semantic-pipes]).

I have put a quick high-level documentation together. Hope it gives a general idea what and how the library will help your Ports and Adapters architecture.

Registry Observer Features
--------------------------
Before going into the small details of `semantic-pipes`, for the purposes of a quick introduction, the notation of set `(a; b)` will be used where `a` or `b` can represent any generic `System.Type`, including types of `IEnumerable`. Where the notations of `a[]` or `b[]` is used, it will mean `IEnumerable<T>`.

Some of the mini-programs that `semantic-pipes` will wire up would be:

- Auto-bridge on intersecting types. For example, if pipes with i/o of sets `(a; b)` and `(b; c)` are registered as pipes, a third pipe will be inserted `(a; c)`. The numbers of pipes will expand as potential bridge-arcs are identified.

- User-defined sets will have priority over auto-bridge pipes. For example, if a set of `(a; c)` was registered by the user in addition to the previous example, then the user-defined set will be used.

- Conversions from stand-alone objects, to enumerable equivalents. For example, if set `(a; b)` is registered by the user, the following sets will be inferred by the `semantic-pipes`:
	- `(a; a[])` - To a single item `IEnumerable`
	- `(b; b[])` - To a single item `IEnumerable`
	- `(a[]; b[])` - foreach item in `a`, it will convert to `b` as per user-defined pipe.

- For each type that `semantic-pipe` observes, it will detect all the contravariance types. For example, if the user registers set `(a; b)`, then the following sets will be registered too:
	- `(a; base_b)`
	- `(a; base_a)`
	- `(b; base_b)`

- All sorts of collections are seen as `IEnumerable`. So if `IEnumerable<T>` is seen in a set, so is `T[]` and so on.

- Every single time a new set `(a; b)` is considered, all observers are able to infer new sets such as `(c; d)`. These inferred sets are then recirculated into the observers to determine if additional inferences may be made, and so on. This is where the most powerful feature of `semantic-pipes` exists.

- Any late `ISemanticRegistryObserver` which is registered late, will receive all sets registered from the beginning of the `SemanticBuilder` life-span.

- Additional observers may be written to assist in the mapping of data types. Think [AutoMapper](http://automapper.org)

Why not just use Dependency Injection?
--------------------------------------
The first skunk works of this pattern indeed use dependency injection. I really enjoyed the flexibility that was afforded by writing small concise `IHandler` based chaining. However, I found the two key limitations.

1. Application start-up was slow. Dependency injection was the problem because it was trying to scan all the assemblies during the startup.
2. There was a massive explosion of classes within the subsystem. Most of the handlers were 4-10 lines long. And quite frankly, grouping these files by namespace was a management nightmare.

I found the dependency injection just too heavy handed, especially when other niche language-like features (such as contravariance) were desirable.

Besides that, I got a little tired of providing hints to the compiler, linker, and the dependency injection to do their thing. And if the application needed to mix synchronous and asynchronous code... yeah, world of micro-management.

That is when I started to see the light with the UNIX way, and the point that I concluded that dependency injection had to go.

Why Support Synchronous and Asynchronous Pipes?
-----------------------------------------------
When I first started to experiment with the pattern with the dependency injection, most of the code was synchronous. The one fruits which I could never easily reach was using the `async` keyword with the 'expensive' thread-hogging operations, such as calling databases (anything on the network is expensive).

With that said, I really wanted to have the ability for synchronous and asynchronous code to weave together in a natural manner. I believe that I have achieved that result.

Further Inspiration
-------------------
Apart from being inspired by the UNIX way on how to chain multiple mini programs together, the [initial inspiration came from Jimmy Bogard](http://lostechies.com/jimmybogard/2013/12/19/put-your-controllers-on-a-diet-posts-and-commands/).

Jimmy has subsequently released [MediatR](http://lostechies.com/jimmybogard/2014/12/17/mediatr-hits-1-0/). There are a few decisions which I do not agree to, such as the dependencies on DI-containers and the multiple registrations of handler classes. But this did motivate me to tidy up the source code and start advertising `semantic-pipe`.