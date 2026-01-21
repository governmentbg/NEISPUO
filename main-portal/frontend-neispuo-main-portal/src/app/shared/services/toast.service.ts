import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

export type ToastStyle = 'info' | 'error' | 'success';
export type ToastPositions = 'top-right' | 'top-left' | 'top-center' | 'bottom-right' | 'bottom-left' | 'bottom-center';

export interface Toast {
  toastId?: number;
  title?: string;
  content: string;
  delay?: number;
  show?: boolean;
  sticky?: boolean;
  closable?: boolean;
  position?: ToastPositions;
  style?: ToastStyle;
  timeoutID?;
}

@Injectable({
  providedIn: 'root',
})
export class ToastService {
  toastArray: Toast[] = [];
  public open = new Subject<Toast[]>();
  nextToastId:number = 0;

  initiate(data: Toast) {
    if(!data.position) 
      data.position = 'top-right'
    if(!data.style) 
      data.style = 'info'

    this.toastArray.push( { ...data, toastId:this.nextToastId, show: true });
    this.nextToastId++;
    this.open.next(this.toastArray);
  }

  hide(id:number) { 
    this.toastArray.map((toast:Toast, i) => {
       if(toast.toastId === id)  
        this.toastArray.splice(i,1);
    })

    this.open.next(this.toastArray);
  }
}
