namespace OsuCzechiaBot.Constants;

public static class HtmlResponses
{
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
                                                <html>
                                                    <body>
                                                        <p>You are already authorized. If you'd like to link a different osu! account, use the <b>/unlink</b> command first.</p>
                                                    </body>
                                                </html>
                                                """;
    
    public const string AuthSomethingWentWrong= """
                                                <html>
                                                    <body>
                                                        <p>Something went wrong when getting your osu data. Please try again later or contact <b>dalibor</b> on Discord!</p>
                                                    </body>
                                                </html>
                                                """;
}