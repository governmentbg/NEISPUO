import { Injectable } from '@angular/core';
import { NeispuoCategory } from '@portal/neispuo-modules/neispuo-category.interface';
import { NeispuoModuleService } from '@portal/neispuo-modules/neispuo-module.service';
import { NeispuoUserGuide } from '@portal/neispuo-modules/neispuo-user-guide.interface';
import { ApiService } from '@shared/services/api.service';
import { ToastService } from '@shared/services/toast.service';
import { saveAs } from 'file-saver';
import { SubSink } from 'subsink';

@Injectable()
export class UserGuideManagementService {
  userGuides: NeispuoUserGuide[];
  subSink = new SubSink();

  constructor(
    private apiService: ApiService,
    private nmService: NeispuoModuleService,
    private toastService: ToastService
  ) {}

  getAllUserGuides() {
    return this.apiService.get('/v1/user-guides-menu');
  }

  getUserGuideByID(id: number) {
    return this.apiService.get(`/v1/user-guides-menu/${id}`);
  }

  addUserGuide(formData: FormData) {
    return this.apiService.post('/v1/user-guides-menu', formData);
  }

  updateUserGuideByID(id: number, formData: FormData) {
    return this.apiService.put(`/v1/user-guides-menu/${id}`, formData);
  }

  deleteUserGuideByID(id: number) {
    return this.apiService.delete(`/v1/user-guides-menu/${id}`);
  }

  downloadUserGuide(userGuide: NeispuoUserGuide) {
    const { id, filename } = userGuide;

    if (!filename) {
      this.toastService.initiate({
        content: 'Ръководството няма прикачен файл.',
        style: 'error',
        sticky: false,
        position: 'bottom-right'
      });

      return;
    }

    this.nmService.downloadFile(id).subscribe(
      (blob) => {
        saveAs(blob, `${filename}`);
      },
      (err) => {
        console.error(err);
      }
    );
  }

  copyUserGuideLink(userGuide: NeispuoUserGuide) {
    const { URLOverride } = userGuide;

    navigator.clipboard
      .writeText(encodeURI(`${URLOverride}`.replace(/\(.+\)/g, '')))
      .then(() => {
        this.toastService.initiate({
          content: 'Връзката е копирана.',
          style: 'success',
          sticky: false,
          position: 'bottom-right'
        });
      })
      .catch(() => {
        this.toastService.initiate({
          content: 'Грешка при копиране на връзката.',
          style: 'error',
          sticky: false,
          position: 'bottom-right'
        });
      });
  }

  loadUserGuides(categories: NeispuoCategory[]) {
    let combinedUserGuides = Object.values(categories).reduce((acc, item: NeispuoCategory) => {
      const { name, id, userGuides } = item;
      const userGuidesWithCategory = userGuides.map((guide) => ({
        ...guide,
        category: {
          name,
          id
        }
      }));
      return acc.concat(userGuidesWithCategory);
    }, []);
    this.userGuides = combinedUserGuides.sort((a: NeispuoUserGuide, b: NeispuoUserGuide) => a.id - b.id);
  }
}
