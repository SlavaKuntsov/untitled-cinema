import { Routes } from '@angular/router'
import { HomePageComponent } from './pages/home-page/home-page.component'
import { NotFoundPageComponent } from './pages/not-found-page/not-found-page.component'
import { ProfileComponent } from './shared/ui/components/profile/profile.component'

export const routes: Routes = [
	{
		path: '',
		component: HomePageComponent
	},
	{
		path: 'profile',
		component: ProfileComponent
	},
	{
		path: '**',
		component: NotFoundPageComponent
	}
]
