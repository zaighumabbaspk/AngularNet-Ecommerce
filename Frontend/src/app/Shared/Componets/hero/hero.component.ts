import { AfterViewInit, Component } from '@angular/core';

@Component({
  selector: 'app-hero',
  standalone: true,
  imports: [],
  templateUrl: './hero.component.html',
  styleUrl: './hero.component.css'
})
export class HeroComponent implements AfterViewInit {


  ngAfterViewInit() {
  const texts = [
    "Transform Your Living Space",
    "Luxury for Every Corner",
    "Designs That Inspire Comfort"
  ];
  let index = 0;
  setInterval(() => {
    const textElement = document.getElementById('changingText');
    if (textElement) {
      textElement.classList.add('fade-out');
      setTimeout(() => {
        index = (index + 1) % texts.length;
        textElement.textContent = texts[index];
        textElement.classList.remove('fade-out');
      }, 600);
    }
  }, 3000);
}


}
