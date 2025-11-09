import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-testimonials',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './testimonials.component.html',
  styleUrls: ['./testimonials.component.css'],
})
export class TestimonialsComponent {
  testimonials = [
    {
      name: 'Sarah Chen',
      role: 'Tech Enthusiast',
      text: 'The quality and innovation of their products are unmatched. My Quantum Pro X has changed how I work.',
      image: 'https://randomuser.me/api/portraits/women/44.jpg',
      rating: 5,
    },
    {
      name: 'Marcus Johnson',
      role: 'Content Creator',
      text: 'Fast shipping, premium packaging, and outstanding customer service. This is my go-to tech store.',
      image: 'https://randomuser.me/api/portraits/men/36.jpg',
      rating: 5,
    },
    {
      name: 'Emily Roberts',
      role: 'Designer',
      text: 'Every product feels like it was designed with the future in mind. Absolutely love the ecosystem.',
      image: 'https://randomuser.me/api/portraits/women/68.jpg',
      rating: 5,
    },
  ];
}
