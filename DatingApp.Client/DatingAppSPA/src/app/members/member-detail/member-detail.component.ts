import { AlertifyService } from "./../../_services/alertify.service";
import { UserService } from "./../../_services/user.service";
import { Component, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { User } from "src/app/models/user";
import { TabsetComponent } from "ngx-bootstrap";

@Component({
  selector: "app-member-detail",
  templateUrl: "./member-detail.component.html",
  styleUrls: ["./member-detail.component.css"]
})
export class MemberDetailComponent implements OnInit {
  @ViewChild("memberTabs", { static: true }) memberTabs: TabsetComponent;
  user: User;

  constructor(
    private userService: UserService,
    private alertifyService: AlertifyService,
    private route: ActivatedRoute
  ) {}
  ngOnInit() {
    this.loadUser();
  }

  loadUser() {
    this.userService.getUser(this.route.snapshot.params["id"]).subscribe(
      (user: User) => {
        this.user = user;
        console.log(user);
      },
      error => {
        this.alertifyService.error(error);
      }
    );
  }
}
