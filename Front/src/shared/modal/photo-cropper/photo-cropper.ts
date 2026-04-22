import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  inject,
  Output,
  ViewChild,
} from '@angular/core';
import { ImageCroppedEvent, ImageCropperComponent, LoadedImage } from 'ngx-image-cropper';

/**
 * ImageUploadComponent: A standalone component for professional image cropping.
 * Optimized for Blob handling to ensure high performance and API compatibility.
 */
@Component({
  selector: 'app-image-upload',
  standalone: true,
  imports: [CommonModule, ImageCropperComponent],
  templateUrl: './photo-cropper.html',
})
export class ImageUploadComponent {
  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;
  @Output() imageReady = new EventEmitter<File>();

  private cdr = inject(ChangeDetectorRef);

  showModal = false;
  imageChangedEvent: any = '';
  croppedBlob: Blob | null = null;

  transform: { scale?: number; rotate?: number } = {};
  rotation = 0;

  // فتح نافذة اختيار الملفات
  triggerUpload(): void {
    this.fileInput.nativeElement.click();
  }

  onFileSelected(event: any): void {
    if (event.target.files && event.target.files.length > 0) {
      this.imageChangedEvent = event;
      this.showModal = true;
      this.rotation = 0;
      this.transform = {};
      this.cdr.detectChanges(); // ضمان ظهور الـ Modal فوراً
    }
  }

  /**
   * استلام بيانات القص كـ Blob (أخف وأسرع للمتصفح) [2026-03-25]
   */
  onImageCropped(event: ImageCroppedEvent): void {
    this.croppedBlob = event.blob || null;

    if (this.croppedBlob) {
      const sizeInKb = (this.croppedBlob.size / 1024).toFixed(2);
      console.log(`Current Crop Size: ${sizeInKb} KB`);
    }
  }

  // التحكم في التدوير
  rotateLeft(): void {
    this.rotation--;
    this.updateTransform();
  }

  rotateRight(): void {
    this.rotation++;
    this.updateTransform();
  }

  private updateTransform(): void {
    this.transform = {
      ...this.transform,
      rotate: this.rotation,
    };
  }

  /**
   * تأكيد القص وإرسال الملف النهائي للهيدر
   */
  confirmUpload(): void {
    if (this.croppedBlob) {
      // تحويل الـ Blob إلى File كائن جاهز للـ API (Multipart/form-data)
      const file = new File([this.croppedBlob], 'profile_photo.jpg', {
        type: 'image/jpeg',
        lastModified: Date.now(),
      });

      console.log('✅ Emitting Final File:', file.name, `${(file.size / 1024).toFixed(2)} KB`);

      this.imageReady.emit(file);
      this.closeModal();
    } else {
      console.error('❌ No Blob data generated. Please adjust the crop area.');
    }
  }

  cancelUpload(): void {
    this.closeModal();
  }

  private closeModal(): void {
    this.showModal = false;
    this.resetFileInput();
    this.cdr.detectChanges();
  }

  private resetFileInput(): void {
    if (this.fileInput) {
      this.fileInput.nativeElement.value = '';
    }
    this.imageChangedEvent = '';
    this.croppedBlob = null;
  }

  onImageLoaded(image: LoadedImage) {
    console.log('Image successfully loaded into cropper');
  }

  onLoadImageFailed() {
    console.error('Failed to load image. Check file format.');
  }
}
