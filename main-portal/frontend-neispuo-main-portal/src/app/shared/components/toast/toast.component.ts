import { Component, OnInit } from '@angular/core';
import { Toast, ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-toast',
  templateUrl: './toast.component.html',
  styleUrls: ['./toast.component.scss'],
})
export class ToastComponent implements OnInit {
  readonly millisecondsToDelay = 3000;
  positions:string[] = ['top-right', 'top-left', 'top-center', 'bottom-right', 'bottom-left', 'bottom-center']

  constructor(public toastService: ToastService) {
    this.toastService.open.subscribe((dataArray) =>
    dataArray.filter((data:Toast) => data.show && !data.sticky && !data.timeoutID)
    .forEach((data:Toast) => this.countDown(data.toastId)));
  }

  ngOnInit() {}

  countDown(toastId:number, resumeTimeout:boolean=false) {
    let toast:Toast = this.toastService.toastArray.filter((toast:Toast) => toast.toastId === toastId)[0]

    if(!toast.sticky && (!toast.timeoutID || resumeTimeout)) {
      toast.timeoutID = setTimeout(() => {
        this.toastService.hide(toastId);
      }, toast.delay ? toast.delay : this.millisecondsToDelay);
    }
  }

  stopCountDown(toastId: number) {
    this.toastService.toastArray.filter((toast:Toast) => toast.toastId === toastId).forEach((toast:Toast) =>
      clearTimeout(toast.timeoutID)
    )
  }

  getToastsByPosition(position:string) {
    return this.toastService.toastArray.filter((toast:Toast) => toast.position === position)
  }
}
