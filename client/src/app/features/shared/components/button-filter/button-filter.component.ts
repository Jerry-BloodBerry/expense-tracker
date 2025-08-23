import { Component, EventEmitter, input, Output } from '@angular/core';
import { Button, ButtonModule } from "primeng/button";
import { Overlay } from 'primeng/overlay';
import { RippleModule } from 'primeng/ripple';

@Component({
  selector: 'app-button-filter',
  imports: [Button, Overlay, ButtonModule, RippleModule],
  templateUrl: './button-filter.component.html',
  styleUrl: './button-filter.component.scss'
})
export class ButtonFilterComponent {
  public visible: boolean = false;
  public title = input.required<string>();
  public label = input.required<string>();
  public icon = input<string>();

  @Output() applyClick = new EventEmitter<void>();

  toggleVisibility(): void {
    this.visible = !this.visible;
  }

  onApply(): void {
    this.applyClick.emit();
  }
}
