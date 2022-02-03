# Blazor Chat

[![SC2 Video](https://raw.githubusercontent.com/DavidEggenberger/BlazorChat/master/MLSA.PNG)](https://www.youtube.com/watch?v=BZcygoYF0pQ&ab_channel=DavidSeesSharp "Click to Watch a walkthrough")

Blazor WebAssembly chat app with an ASP.NET Core backend. Users can login with social providers (LinkedIn, GitHub) and can configure the login experience by enabling 2 Factor Authentcation (Authenticator App, Google Authenticator for example). When the user is logged in he/she can chat with fellow loggedin users that appear in their own diagrams. Once the connection is made (dragging the mouse to the node of the recipient) the users can chat. SignalR is used for realtime chatting. IdentityServer is used to issue the Access and Id tokens. 
