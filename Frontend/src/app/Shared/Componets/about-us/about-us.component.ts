import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-about-us',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './about-us.component.html',
  styleUrls: ['./about-us.component.css']
})
export class AboutUsComponent {
  teamMembers = [
    {
      name: 'John Anderson',
      role: 'Founder & CEO',
      image: 'https://via.placeholder.com/300x300?text=John+Anderson',
      bio: 'Visionary leader with 20 years in luxury furniture design.'
    },
    {
      name: 'Sarah Mitchell',
      role: 'Design Director',
      image: 'https://via.placeholder.com/300x300?text=Sarah+Mitchell',
      bio: 'Award-winning designer creating timeless aesthetic collections.'
    },
    {
      name: 'Michael Chen',
      role: 'Operations Manager',
      image: 'https://via.placeholder.com/300x300?text=Michael+Chen',
      bio: 'Expert in supply chain management and quality assurance.'
    },
    {
      name: 'Emma Rodriguez',
      role: 'Customer Success Lead',
      image: 'https://via.placeholder.com/300x300?text=Emma+Rodriguez',
      bio: 'Dedicated to ensuring exceptional customer experiences.'
    }
  ];

  values = [
    {
      icon: '⭐',
      title: 'Premium Quality',
      description: 'We source the finest materials and employ master craftsmen to create furniture that lasts generations.'
    },
    {
      icon: '🌱',
      title: 'Sustainability',
      description: 'Eco-friendly manufacturing processes and responsibly sourced materials are at the heart of our production.'
    },
    {
      icon: '💡',
      title: 'Innovation',
      description: 'Continuous design evolution blending timeless aesthetics with modern functionality.'
    },
    {
      icon: '❤️',
      title: 'Customer Focus',
      description: 'Your satisfaction is our priority. Personalized service for every customer journey.'
    }
  ];
}
