# Blazor Chat

<img src="https://raw.githubusercontent.com/DavidEggenberger/BlazorChat/master/MLSA.PNG"/>

Blazor WebAssembly chat app with an ASP.NET Core backend. Users can login with social providers (LinkedIn, GitHub) and can configure the login experience by enabling 2 Factor Authentcation (Authenticator App, Google Authenticator for example). When the user is logged in he/she can chat with fellow loggedin users that appear in their own diagrams. Once the connection is made (dragging the mouse to the node of the recipient) the users can chat. SignalR is used for realtime chatting. IdentityServer is used to issue the Access and Id tokens. 
