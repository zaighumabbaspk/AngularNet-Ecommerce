import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CustomNotificationService } from '../../../Core/Services/custom-notification.service';

@Component({
  selector: 'app-about-us',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './about-us.component.html',
  styleUrls: ['./about-us.component.css']
})
export class AboutUsComponent implements OnInit {
  constructor(private notification: CustomNotificationService) {}

  ngOnInit(): void {
  }
  teamMembers = [
    {
      name: 'John Anderson',
      role: 'Founder & CEO',
      image: 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=400&h=400&fit=crop',
      bio: 'Visionary leader with 20 years in luxury furniture design. Pioneered the modern luxury furniture movement.',
      specialty: 'Strategic Vision'
    },
    {
      name: 'Sarah Mitchell',
      role: 'Design Director',
      image: 'https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=400&h=400&fit=crop',
      bio: 'Award-winning designer creating timeless aesthetic collections. Multiple international design awards.',
      specialty: 'Creative Design'
    },
    {
      name: 'Michael Chen',
      role: 'Operations Manager',
      image: 'https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=400&h=400&fit=crop',
      bio: 'Expert in supply chain management and quality assurance. Ensures excellence in every delivery.',
      specialty: 'Operations'
    },
    {
      name: 'Emma Rodriguez',
      role: 'Customer Success Lead',
      image: 'https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=400&h=400&fit=crop',
      bio: 'Dedicated to ensuring exceptional customer experiences. 15+ years in luxury customer service.',
      specialty: 'Customer Care'
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

  contactUs(): void {
    this.notification.success(
      'Contact request submitted! We will get back to you soon.',
      'Thank You!'
    );
  }
}
