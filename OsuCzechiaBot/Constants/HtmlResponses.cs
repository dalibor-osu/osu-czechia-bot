namespace OsuCzechiaBot.Constants;

public static class HtmlResponses
{
    public const string AuthBeingProcessed = $"""
                                             <html>
                                             {Head}
                                                 <body>
                                                     <p>You are being authorized. You will be pinged on the server once the authorization is done.</p>
                                                     <p>This page should close automatically when authorization is done.</p>
                                             """;

    public const string AuthDone = """
                                        <script type='text/javascript'>
                                            window.close();
                                        </script>
                                        </body>
                                    </html>
                                   """;

    public const string AuthFailed = """
                                            <p>Something went wrong with your authorization. Try again later or message @dalibor on Discord.</p>
                                        </body>
                                    </html>
                                   """;

    public const string AuthSuccess = """
                                      <html>
                                          <body>
                                              <script type='text/javascript'>
                                                  window.close();
                                              </script>
                                              <p>If your tab does not close automatically, please close it manually.</p>
                                          </body>
                                      </html>
                                      """;

    public const string AuthAlreadyAuthorized = """
                                                        <p>You are already authorized. If you'd like to link a different osu! account, use the <b>/unlink</b> command first.</p>
                                                    </body>
                                                </html>
                                                """;
    
    private const string Head = $"""
                                <head>
                                <meta charset="UTF-8">
                                <title>osu! Czechia authorization</title>
                                {SimpleCss}
                                </head>
                                """;
    
    private const string SimpleCss = """
                                     <style>
                                       body {
                                         font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                                         line-height: 1.6;
                                         background-color: #f9f9f9;
                                         color: #333;
                                         padding: 20px;
                                         max-width: 700px;
                                         margin: 0 auto;
                                       }
                                     
                                       p {
                                         margin-bottom: 15px;
                                       }
                                     </style>
                                     """;
}