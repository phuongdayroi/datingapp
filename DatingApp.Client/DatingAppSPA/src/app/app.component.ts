import { RouterModule } from "@angular/router";
import { AlertifyService } from "./_services/alertify.service";
import { AuthService } from "./_services/auth.service";
import { Component } from "@angular/core";
import { JwtHelperService } from "@auth0/angular-jwt";
import { Route } from "@angular/compiler/src/core";

@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.css"]
})
export class AppComponent {
  title = "DatingAppSPA";
  jwtHelper = new JwtHelperService();

  constructor(private authService: AuthService) {}

  ngOnInit() {
    const token = localStorage.getItem("token");
    if (token) {
      this.authService.decodedToken = this.jwtHelper.decodeToken(token);
    }
  }
}
